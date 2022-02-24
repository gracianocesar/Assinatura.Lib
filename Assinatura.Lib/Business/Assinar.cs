using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.security;
using Assinador.Lib.Exceptions;
using Assinador.Lib.Model;

namespace Assinador.Lib.Business
{
    public static class Assinar
    {
        public static List<Certificado> ObterCertificadosUsuarioCorrente(Func<Certificado, bool> filter = null)
        {
            var stores = new Dictionary<StoreName, string>()
            {
                {StoreName.My, "Personal"},
                {StoreName.Root, "Trusted roots"},
                {StoreName.TrustedPublisher, "Trusted publishers"}
            }.Select(s => new { store = new X509Store(s.Key, StoreLocation.CurrentUser), location = s.Value })
            .ToArray();

            foreach (var store in stores)
            {
                store.store.Open(OpenFlags.ReadOnly);
            }

            var certificates = stores.SelectMany(s => s.store.Certificates.Cast<X509Certificate2>()
                .Select(certificate => new Certificado
                {
                    SubjectName = certificate.SubjectName.Name,
                    Oid = certificate.SubjectName.Oid.ToString(),
                    Location = s.location,
                    Issuer = certificate.Issuer,
                    Certificate = certificate,
                    Owner = certificate.Subject.Split(',').Select(x => x.Split('=')[1]).First(),
                    HasPrivateKey = certificate.HasPrivateKey
                }))
                .Where(x => DateTime.Now >= x.Certificate.NotBefore && DateTime.Now <= x.Certificate.NotAfter)
                .Where(x => x.HasPrivateKey)
                .Where(x => x.Owner.Contains(":"));

            if (filter != null)
            {
                certificates = certificates.Where(filter);
            }
            return certificates.ToList();
        }

        public static byte[] AssinarArquivo(string subjectName, byte[] data)
        {
            var certificado = ObterCertificadosUsuarioCorrente(x => x.Owner == subjectName).FirstOrDefault().Certificate;
            var mime = MimeType.GetMimeFromFile(data);
            if (mime.Equals("application/pdf"))
            {
                return AssinarArquivoPAdES(certificado, data);
            }
            return AssinarArquivo(certificado, data);
        }

        public static byte[] AssinarArquivo(X509Certificate2 certificado, byte[] data)
        {
            var mime = MimeType.GetMimeFromFile(data);
            if (mime.Equals("application/pdf"))
            {
                return AssinarArquivoPAdES(certificado, data);
            }
            try
            {
                var contentInfo = new ContentInfo(data);
                var signedCms = new SignedCms(contentInfo, false);

                if (ArquivoNaoAssinadoAnteriormente(data))
                {
                    signedCms.Decode(data);
                }

                var signer = new CmsSigner(SubjectIdentifierType.IssuerAndSerialNumber, certificado)
                {
                    IncludeOption = X509IncludeOption.WholeChain
                };

                signer.SignedAttributes.Add(new Pkcs9SigningTime());

                signedCms.ComputeSignature(signer, false);

                return signedCms.Encode();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Erro ao assinar arquivo. A mensagem retornada foi: {ex.Message}");
            }
        }

        public static byte[] AssinarArquivoPAdES(X509Certificate2 certificado, byte[] data)
        {
            try
            {
                var cp = new Org.BouncyCastle.X509.X509CertificateParser();
                var chain = new Org.BouncyCastle.X509.X509Certificate[] { cp.ReadCertificate(certificado.RawData) };

                IExternalSignature externalSignature = new X509Certificate2Signature(certificado, "SHA-1");
                PdfReader pdfReader = new PdfReader(data);
                var ms = new MemoryStream();

                var pdfStamper = PdfStamper.CreateSignature(pdfReader, ms, '\0', null, true);

                PdfSignatureAppearance signatureAppearance = pdfStamper.SignatureAppearance;
                signatureAppearance.CertificationLevel = PdfSignatureAppearance.NOT_CERTIFIED;

                MakeSignature.SignDetached(signatureAppearance, externalSignature, chain, null, null, null, 0, CryptoStandard.CMS);

                return ms.ToArray();
            }
            catch
            {
                throw new BusinessException("Erro ao assinar arquivo PDF.");
            }
        }

        public static List<Assinatura> ObterAssinaturasDoArquivo(byte[] data)
        {
            var tempFile = CriarArquivoTmp(data);
            var mime = MimeType.GetMimeFromFile(tempFile);
            if (mime.Equals("application/pdf"))
            {
                return ObterAssinaturaPAdES(tempFile);
            }
            var assinaturas = new List<Model.Assinatura>();
            try
            {
                var signed = new SignedCms();
                signed.Decode(data);

                foreach (var signerInfo in signed.SignerInfos)
                {
                    var info = signerInfo.Certificate.Subject.Split(',')
                                                             .Select(x => x.Split('=')[1]).First()
                                                             .Split(':');
                    var dataAssinatura = string.Empty;

                    var attributes = signerInfo.SignedAttributes;

                    if (attributes.Count > 1)
                    {
                        var values = attributes[1].Values;
                        if (values.Count > 0 && values[0] is Pkcs9SigningTime time)
                        {
                            var date = time.SigningTime;
                            dataAssinatura = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(date, "E. South America Standard Time").ToString("dd/MM/yyyy HH:mm:ss");
                        }
                    }
                    assinaturas.Add(new Model.Assinatura(info.ElementAtOrDefault(0), info.ElementAtOrDefault(1), dataAssinatura, signerInfo.Certificate.Subject));
                }
                return assinaturas;
            }
            catch
            {
                throw new BusinessException("Arquivo não assinado eletronicamente.");
            }
        }
        public static bool PDFAssinado(byte[] data)
        {
            PdfReader pdfReader = new PdfReader(data);
            AcroFields fields = pdfReader.AcroFields;
            List<string> names = fields.GetSignatureNames();
            var assinado = false;
            foreach (var name in names)
            {
                PdfPKCS7 pkcs7 = fields.VerifySignature(name);
                assinado = pkcs7.Verify();
            }
            return assinado;
        }
        public static List<Assinatura> ObterAssinaturaPAdES(byte[] data)
        {
            var assinaturas = new List<Model.Assinatura>();
            PdfReader pdfReader = new PdfReader(data);
            AcroFields fields = pdfReader.AcroFields;
            List<string> names = fields.GetSignatureNames();

            foreach (var name in names)
            {
                PdfPKCS7 pkcs7 = fields.VerifySignature(name);
                var certificates = pkcs7.Certificates;
                foreach (var cert in certificates)
                {
                    var infos = cert.SubjectDN;
                    foreach (var info in infos.GetValueList(Org.BouncyCastle.Asn1.X509.X509Name.CN))
                    {
                        var dados = info.ToString().Split(':');
                        assinaturas.Add(new Model.Assinatura(dados.ElementAtOrDefault(0), dados.ElementAtOrDefault(1), pkcs7.SignDate.ToString(), cert.SubjectDN.ToString()));
                    }
                }
            }

            if (assinaturas.Count > 0)
            {
                return assinaturas;
            }
            throw new BusinessException("Arquivo não assinado eletronicamente.");
        }

        public static bool ArquivoNaoAssinadoAnteriormente(byte[] data)
        {
            try
            {
                var signed = new SignedCms();
                signed.Decode(data);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static byte[] ObterArquivoDecodificado(byte[] data)
        {
            try
            {
                var mime = MimeType.GetMimeFromFile(data);
                if (mime.Equals("application/pdf"))
                {
                    return data;
                }
                var signed = new SignedCms();
                signed.Decode(data);

                return signed.ContentInfo.Content;
            }
            catch
            {
                throw new BusinessException("Arquivo não assinado ou inválido.");
            }
        }

        public static byte[] CriarArquivoTmp(byte[] arquivoTmp)
        {
            var pathTmp = Path.GetTempFileName();
            File.WriteAllBytes(pathTmp, arquivoTmp);
            return File.ReadAllBytes(pathTmp);
        }
    }
}
