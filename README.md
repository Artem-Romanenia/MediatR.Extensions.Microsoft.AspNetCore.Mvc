# MediatR.Extensions.Microsoft.AspNetCore.Mvc
[MediatR.Extensions.Microsoft.AspNetCore.Mvc](https://www.nuget.org/packages/MediatR.Extensions.Microsoft.AspNetCore.Mvc) is the extension that marries MediatR to ASP.NET Core in a way that keeps MediatR totally agnostic of how it is being used.

Easily configurable, it wraps each MediatR request in a constructed generic controller and provides several flexibility points.

If you have a lot of actions like this...

``` csharp
public async Task<IActionResult> MyRequestAction(MyRequest request)
{
    var response = await _mediator.Send(request);

    return Json(response);
}
```

... and you want to get rid of them, you may very well give this extension a try.

See [Wiki](https://github.com/Artem-Romanenia/MediatR.Extensions.Microsoft.AspNetCore.Mvc/wiki) to get started.
