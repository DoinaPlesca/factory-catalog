using api.Helper;
using api.Middleware;
using infrastructure;
using infrastructure.Repository;
using service;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
        dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString);
}

builder.Services.AddSingleton<BoxRepository>();
builder.Services.AddSingleton<BoxService>();
builder.Services.AddSingleton<ResponseHelper>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var frontEndRelativePath = "./../appfrontend/www";

builder.Services.AddSpaStaticFiles(conf => conf.RootPath = frontEndRelativePath);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin",
        builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});

var app = builder.Build();



// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});


app.UseSpaStaticFiles();
app.UseSpa(conf =>
{
    conf.Options.SourcePath = frontEndRelativePath;
});


app.MapControllers();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();