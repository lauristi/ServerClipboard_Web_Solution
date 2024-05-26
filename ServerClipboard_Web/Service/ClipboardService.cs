using Microsoft.JSInterop;

namespace ServerClipboard_Web.Service
{
    //-------------------------------------------------------------------------------------
    // https://www.meziantou.net/copying-text-to-clipboard-in-a-blazor-application.htm
    //-------------------------------------------------------------------------------------

    public sealed class ClipboardService
    {
        private readonly IJSRuntime _jsRuntime;

        public ClipboardService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public ValueTask<string> ReadTextAsync()
        {
            return _jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
        }

        public ValueTask WriteTextAsync(string text)
        {
            return _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
        }
    }
}