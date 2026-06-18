namespace Messenger.API.Contracts.Profile;

public sealed record ConfirmPhoneNumberChangeRequest(string PhoneNumber, string Code);
