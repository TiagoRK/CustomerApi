using CustomerApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGlobalization(builder.Configuration);

// Existing registrations are preserved via the partial Program pattern;
// this file hooks globalization in alongside the rest of startup.
