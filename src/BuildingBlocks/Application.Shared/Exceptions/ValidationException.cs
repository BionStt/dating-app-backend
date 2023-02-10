namespace Application.Shared.Exceptions
{
       /**
        * 
        *  
        * Take from the article CQRS ValidationPipeline With MediatR and FluentValidation
        * https://code-maze.com/cqrs-mediatr-fluentvalidation/
        * 
        * **/
    public sealed class ValidationException : ApiException
    {
        public ValidationException(IReadOnlyDictionary<string, string[]> errorsDictionary) : base("Validation Failure", "One or more validation errors occurred")
        {
            ErrorsDictionary = errorsDictionary;
        }

        public IReadOnlyDictionary<string, string[]> ErrorsDictionary { get; }

    }
}
