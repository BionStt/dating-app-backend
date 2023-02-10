
namespace Application.Shared.Exceptions
{
    /**
     * 
     *  
     * Take from the article CQRS ValidationPipeline With MediatR and FluentValidation
     * https://code-maze.com/cqrs-mediatr-fluentvalidation/
     * 
     * **/
    public abstract class ApiException : Exception
    {
        protected ApiException(string title, string message) : base(message) => Title = title;
        public string Title { get; }
    }
}
