using Microsoft.AspNetCore.Mvc;

namespace GIBS.Media.Images;

[ApiController]
[Route($"/{PathFragment}")]
public class ImagesController : ControllerBase
{
    internal const string PathFragment = "image";

    [HttpPost]
    public virtual async Task Handle([FromServices] ImageHandler_ imageHandler)
        => await imageHandler.HandleAsync();
}
