using Front.Services;

var builder = WebApplication.CreateBuilder(args);
var urlGateway = "https://localhost:7181"; // URL du Gateway


// Configuration des dépendances
builder.Services.AddControllersWithViews();


// Session (pour stocker le JWT + RefreshToken)
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


// HttpContextAccessor (pour accéder à la session dans les services)
builder.Services.AddHttpContextAccessor();


// HttpClient pour appeler le Gateway
builder.Services.AddHttpClient<PatientApiService>(client =>
{
    client.BaseAddress = new Uri(urlGateway);
});

builder.Services.AddHttpClient<NoteApiService>(client =>
{
    client.BaseAddress = new Uri(urlGateway);
});

builder.Services.AddHttpClient<RisqueApiService>(client =>
{
    client.BaseAddress = new Uri(urlGateway);
});

builder.Services.AddHttpClient("GatewayClient", client =>
{
    client.BaseAddress = new Uri(urlGateway);
});


// Construction de l'application
var app = builder.Build();


// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}")
    .WithStaticAssets();

app.Run();
