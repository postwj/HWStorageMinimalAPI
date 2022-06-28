using Microsoft.EntityFrameworkCore;
using HWStorageAPI.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DeviceContext>(opt => opt.UseInMemoryDatabase("DeviceLisat"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/fuck", () => "Fuck You!");

app.MapGet("/devices", async (DeviceContext db) =>
    await db.Devices.ToListAsync());

app.MapGet("/devices/hasserial", async (DeviceContext db) => 
    await db.Devices.Where(x => !string.IsNullOrEmpty(x.SerialNumber)).ToListAsync());

app.MapGet("/devices/{id}", async (int id, DeviceContext db) => 
    await db.Devices.FindAsync(id)// is Device device ? Results.Ok(device) : Results.NotFound()
            is Device device
            ? Results.Ok(device)
            : Results.NotFound()
);

app.MapPost("/devices", async (Device device, DeviceContext db) => 
{
    db.Devices.Add(device);
    await db.SaveChangesAsync();
    return Results.Created($"/devices/{device.DeviceId}", device);
});

app.MapPut("/devices/{id}", async (int id, Device device, DeviceContext db) => 
{
    var existingDevice = await db.Devices.FindAsync(id);
     
    if(existingDevice is null) return Results.NotFound();

    existingDevice.Name = device.Name;
    existingDevice.SerialNumber = device.SerialNumber;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/devices/{id}", async (int id, DeviceContext db) => 
{
    if (await db.Devices.FindAsync(id) is Device device)
    {
        db.Devices.Remove(device);
        await db.SaveChangesAsync();
        return Results.Ok(device);
    }

    return Results.NotFound();
});


app.Run();