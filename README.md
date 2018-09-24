# MediatR.Extensions.Microsoft.AspNetCore.Mvc
[MediatR.Extensions.Microsoft.AspNetCore.Mvc](https://www.nuget.org/packages/MediatR.Extensions.Microsoft.AspNetCore.Mvc) is an extension that marries MediatR to ASP.NET Core in a way that keeps MediatR totally agnostic of how it is being used, while providing you with tools to shorten the amount of repetitive Mvc controller-related code to a minimun.

Easily configurable, it wraps each MediatR request in a constructed generic controller and provides several flexibility points.

If you have a lot of actions in your controllers reduced to something as rudimentary as...

``` csharp
public async Task<IActionResult> MyRequestAction(MyRequest request)
{
    var response = await _mediator.Send(request);

    return Json(response);
}
```

... and you want to get rid of them, you may very well give this extension a try.

## MediatR version support

MediatR.Extensions.Microsoft.AspNetCore.Mvc version 1.0.0 lists MediatR 4.0.0 as a dependency, but it equally supports every version of MediatR up to 5.1.0

## See [Wiki](https://github.com/Artem-Romanenia/MediatR.Extensions.Microsoft.AspNetCore.Mvc/wiki) to get started.
