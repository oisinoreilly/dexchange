using ServiceStack.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Contracts
{
    public interface IDocuSignService
    {
        string CreateEnvelope(string base64Content, string documentId, string documentName, UserAuth user);
        string GetSignedDocumentContent(string documentId, string envelopeId);
    }
}
