using br.com.fiap.cloudgames.Payment.Domain.Enums;
using br.com.fiap.cloudgames.Payment.Domain.Exceptions;
using br.com.fiap.cloudgames.Payment.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace br.com.fiap.cloudgames.Payment.Domain.Aggregates
{
    public class Payment
    {
        public Payment() { } //ORM

        #region  FactoryMethod
        public static Payment Create(Name name, EmailAddress email, String identityId)
        {
            Validate(name, email, identityId);
            return new Payment()
            {
                Id = Guid.NewGuid(),
                Name = name,
                Email = email,
                UserAccountStatus = UserAccountStatus.ACTIVE,
                IdentityId = identityId,
                CreationDate = DateTime.Now,
                Role = UserRoles.user
            };
        }
        #endregion

        #region  Properties
        //PaymentId
        //OrderId (em caso de extorno, segundo pagamento ou transação mas mesmo pedido)
        public PaymentStatus PaymentStatus { get; private set; }
        #endregion

        private static void Validate(Name name, EmailAddress email, String identityId)
        {
            var errors = new List<string>();

            if (name is null)
                errors.Add("Name is required.");

            if (email is null)
                errors.Add("Email is required.");

            if (String.IsNullOrWhiteSpace(identityId))
                errors.Add("IdentityId is required.");

            if (errors.Any())
                throw new DomainException(errors);
        }
    }
}
