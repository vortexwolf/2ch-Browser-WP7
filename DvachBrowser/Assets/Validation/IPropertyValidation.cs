namespace DvachBrowser.Assets.Validation
{
    public interface IPropertyValidation
    {
        string PropertyName { get; }

        string ErrorMessage { get; }

        bool IsInvalid();
    }
}
