using Messenger.Domain.Common;
using Messenger.Domain.Enums;

namespace Messenger.Domain.Aggregates.Auth
{
    public sealed class OtpCode : AggregateRoot<Guid>
    {
        public string PhoneNumber { get; private set; }
        public string CodeHash { get; private set; }
        public DateTime ExpiresAtUtc { get; private set; }
        public bool IsUsed { get; private set; }
        public OtpPurpose Purpose { get; set; }
        public int Attempts { get; private set; }

        private OtpCode()
        {
        }

        private OtpCode(
            Guid id,
            string phoneNumber,
            string codeHash,
            OtpPurpose purpose,
            DateTime expiresAtUtc) : base(id)
        {
            PhoneNumber = phoneNumber;
            CodeHash = codeHash;
            Purpose = purpose;
            ExpiresAtUtc = expiresAtUtc;
            Attempts = 1;
        }

        public static OtpCode Create(
            string phoneNumber,
            string codeHash,
            OtpPurpose purpose,
            DateTime expiresAtUtc)
        {
            return new OtpCode(Guid.NewGuid(), phoneNumber, codeHash, purpose, expiresAtUtc);
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow >= ExpiresAtUtc;
        }

        public void MarkAsUsed()
        {
            if (IsUsed) return;

            IsUsed = true;
            SetUpdated(null);
        }

        public void IncreaseAttempt()
        {
            Attempts++;
            SetUpdated(null);
        }
    }
}
