using System.IdentityModel.Tokens.Jwt;

namespace SkelbimuIS.Models
{
    public class AuthModel
    {
        public static JwtSecurityToken ConvertJwtStringToJwtSecurityToken(string? jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            return token;
        }

        public static DecodedToken DecodeJwt(JwtSecurityToken token)
        {
            var keyId = token.Header.Kid;
            var audience = token.Audiences.ToList();
            var claims = token.Claims.Select(claim => (claim.Type, claim.Value)).ToList();

            return new DecodedToken(
                keyId,
                token.Issuer,
                audience,
                claims,
                token.ValidTo,
                token.SignatureAlgorithm,
                token.RawData,
                token.Subject,
                token.ValidFrom,
                token.EncodedHeader,
                token.EncodedPayload
            );
        }
    }

    public class DecodedToken
    {
        public string keyId;
        public string issuer;
        public List<string> audience;
        public List<(string Type, string Value)> claims;
        public DateTime validTo;
        public string signatureAlgorithm;
        public string rawData;
        public string subject;
        public DateTime validFrom;
        public string encodedHeader;
        public string encodedPayload;

        public DecodedToken(string keyId, string issuer, List<string> audience, List<(string Type, string Value)> claims, DateTime validTo, string signatureAlgorithm, string rawData, string subject, DateTime validFrom, string encodedHeader, string encodedPayload)
        {
            this.keyId = keyId;
            this.issuer = issuer;
            this.audience = audience;
            this.claims = claims;
            this.validTo = validTo;
            this.signatureAlgorithm = signatureAlgorithm;
            this.rawData = rawData;
            this.subject = subject;
            this.validFrom = validFrom;
            this.encodedHeader = encodedHeader;
            this.encodedPayload = encodedPayload;
        }
    }
}