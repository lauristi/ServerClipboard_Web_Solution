using ServerClipboard_Web.Components;
using ServerClipboard_Web.Service;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

//========================================================================================================
// Configure o Kestrel para ouvir em todas as interfaces de rede na porta 5020
// Adiciona o middleware UseStaticWebAssets para servir arquivos estáticos, incluindo CSS, em produção
//========================================================================================================

if (!builder.Environment.IsDevelopment())
{
    builder.WebHost.UseWebRoot("wwwroot")
                   .UseStaticWebAssets();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Parse("192.168.0.156"), 5021);
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
        BaseAddress = new Uri("http://192.168.0.156:5020")
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