using AAP.Api.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation()
    .AddHttpClients()
    .AddApplicationServices()
    .AddAuthenticationAndAuthorization(builder.Configuration)
    .AddCorsPolicy();



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Allow");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
