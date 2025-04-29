using FinCtrlLibrary.Interfaces;
using FinCtrlLibrary.Validators;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinCtrlLibrary.Models
{
    public class Authentication : ValidatorClass, IMongoDbItem
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public DateTime DateTime { get; set; }
        public string Ip { get; set; }
        public string Token { get; set; }
        public string UserBsonId { get; set; }
        public bool ValidTokenLifetime
        {
            get
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // Ensure the token is well-formed
                if (!tokenHandler.CanReadToken(Token))
                    throw new ArgumentException("Invalid token format.");

                var jwtToken = tokenHandler.ReadJwtToken(Token);
                var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp) ?? throw new ArgumentException("Token does not contain an 'exp' claim.");

                // Get the expiration time as a DateTime
                var exp = long.Parse(expClaim.Value);
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;

                // Compare with the current time
                return expirationTime <= DateTime.UtcNow;
            }
        }
        protected override void Validate()
        {
            ObjectIdValidation(nameof(_id), _id.ToString());
            NotBiggerThanNowDateTimeValidation(nameof(DateTime), DateTime);
            IpValidation(nameof(Ip), Ip);
            NotEmptyStringValidation(nameof(Token), Token);
            ObjectIdValidation(nameof(UserBsonId), UserBsonId);
        }
    }
}
