using ServerClipboard_Web.Components;
using ServerClipboard_Web.Service;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var options = new WebApplicationOptions
{
    WebRootPath = "wwwroot", // já define o caminho correto no início
    Args = args              // importante passar os args aqui também
};

var builder = WebApplication.CreateBuilder(options); // <== USANDO OS OPTIONS AQUI

var configuration = builder.Configuration;

var apiBaseAddress = configuration["ConnectionSettings:ApiBaseAddress"];
var bindAddress = configuration["ConnectionSettings:BindAddress"];
var bindPort = int.Parse(configuration["ConnectionSettings:BindPort"] ?? "5021");

//========================================================================================================
// Configure o Kestrel para ouvir no IP e porta configurados
//========================================================================================================

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseStaticWebAssets();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Parse(bindAddress), bindPort);
    });
}
//========================================================================================================


// Add services to the container.
builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

// SERVIÇOS PARA ACESSO AO CLIPBOARD E  RECUPERAR O IP DO CONTEXTO
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddHttpContextAccessor();

//========================================================================================================
// ENDERECO PARA A ServerClipboard_API (altere de acordo com sua implantação)
//========================================================================================================
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(apiBaseAddress)
    });
//========================================================================================================

var app = builder.Build();

////=============================================================================================
////CODIGO PARA SERVIR ARQUIVOS ESTATICOS FORA DO PADRÃO MIME
////=============================================================================================
//var provider = new FileExtensionContentTypeProvider();
//provider.Mappings["{EXTENSION}"] = "{CONTENT TYPE}";

//app.UseStaticFiles(new StaticFileOptions { ContentTypeProvider = provider });
////=============================================================================================

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//========================================================================================================
//Configuracao de Cabecalho encaminhado para funcionar com proxy reverso... Ngnix
//========================================================================================================
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
//========================================================================================================

//REMOVIDO PARA EVITAR ERRO DE AUTENTICACAO NO  Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider No Raspbery PI
//app.UseAuthentication();

app.Run();