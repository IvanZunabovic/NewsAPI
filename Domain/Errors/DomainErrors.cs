using Domain.Shared;

namespace Domain.Errors;

public static class DomainErrors
{
    public static class Email
    {
        public static readonly Error Empty = new(
            "Email.Empty",
            "Email is empty.");

        public static readonly Error InvalidFormat = new(
            "Email.InvalidFormat",
            "Email format is invalid.");
    }

    public static class FirstName
    {
        public static readonly Error Empty = new(
            "FirstName.Empty",
            "FirstName is empty.");

        public static readonly Error TooLong = new(
            "FirstName.TooLong",
            "FirstName name is too long.");
    }
    
    public static class LastName
    {
        public static readonly Error Empty = new(
            "LastName.Empty",
            "LastName is empty.");

        public static readonly Error TooLong = new(
            "LastName.TooLong",
            "LastName name is too long.");
    }
    public static class Content
    {
        public static readonly Error Empty = new(
            "Content.Empty",
            "Content is empty.");

        public static readonly Error TooLong = new(
            "Content.TooLong",
            "Content is too long.");
    }
}
