using Microsoft.EntityFrameworkCore;
using TicketManager.Common.Interface;
using TicketManager.Common.Models.Entities;
using TicketManager.DAL;
using TicketManager.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration["SqlServerConnectionString"]));
builder.Services.AddOptions();

builder.Services.AddScoped<IRepository<Cart, Guid>, CartRepository>();
builder.Services.AddScoped<IRepository<Event, int>, EventRepository>();
builder.Services.AddScoped<IRepository<Payment, int>, PaymentRepository>();
builder.Services.AddScoped<IRepository<Price, int>, PriceRepository>();
builder.Services.AddScoped<IRepository<Row, int>, RowRepository>();
builder.Services.AddScoped<IRepository<Seat, int>, SeatRepository>();
builder.Services.AddScoped<IRepository<Section, int>, SectionRepository>();
builder.Services.AddScoped<IRepository<User, int>, UserRepository>();
builder.Services.AddScoped<IRepository<Venue, int>, VenueRepository>();

builder.Services.AddScoped<IAppDbContext, AppDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
