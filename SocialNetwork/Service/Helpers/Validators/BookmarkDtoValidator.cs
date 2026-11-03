using Service.Helpers.DTOs.Bookmark;

namespace Service.Helpers.Validators
{
    public class BookmarkDtoValidator : DtoValidator<BookmarkDto>
    {
        public BookmarkDtoValidator()
        {
            RuleFor(bookmark => bookmark.PostId).NotEmpty();
            RuleFor(bookmark => bookmark.UserId).NotEmpty();
        }
    }
}
