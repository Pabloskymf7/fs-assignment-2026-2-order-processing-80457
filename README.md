# SportsStore – ASP.NET Core (.NET 10)
Pablo Miguel Ferrer-80457
Finished 25/02/2026

I created the branch **pmf-upgrade-john-suggestions-05032026** after finishing the project to add the requirements that John sent us by email on 05/03/2026, but the project was actually completed on 25/02/2026.

## Upgrade Steps

1. Changed TargetFramework to net10.0 in both SportsStore.csproj and SportsStore.Tests.csproj.

2. Updated all NuGet packages to versions compatible with .NET 10. In the main project: EF Core, Identity, SqlClient and System.Drawing.Common. In the test project: xunit, Moq, coverlet, the test SDK, SqlClient and System.Drawing.Common.

3. Synchronised package versions between projects. Since the test project references the main project, NuGet resolves all dependencies together. If one project requested a different version of the same package, a downgrade error occurred. The fix was to declare the same highest version explicitly in both projects.

4. Updated test method calls. The HomeController.Index method signature changed to accept a category and a page number. Tests that called Index(2) were updated to Index(null, 2).

5. Registered Cart as a session service in Program.cs using AddSession(), AddScoped and UseSession().

## Stripe Configuration

Stripe is integrated using the official Stripe.net SDK.

API keys are never stored in code or appsettings.json. They are stored using .NET User Secrets. To set them up, right-click the SportsStore project in Visual Studio and click Manage User Secrets, then add:

{
  "Stripe:SecretKey": "sk_test_...",
  "Stripe:PublishableKey": "pk_test_..."
}

Payment flow:
- User fills in shipping details and submits checkout
- A PaymentIntent is created server-side
- User enters card details in the Stripe card element
- On success the server verifies the payment and saves the order
- User is redirected to the confirmation page
- Failed or cancelled payments redirect to dedicated error pages

Test cards:
- 4242 4242 4242 4242 - payment succeeds
- 4000 0000 0000 0002 - payment declined
Use any future expiry date and any 3-digit CVC.

## Logging Setup

Logging uses Serilog configured via appsettings.json.

Logs are written to the console, to a daily rolling file in the Logs/ folder, and optionally to Seq at http://localhost:5341.

Log levels by environment:
- Development: Debug minimum
- Production: Warning minimum

What gets logged:
- Application startup
- Every HTTP request with method, path, status code and duration
- Checkout attempted with empty cart
- Payment intent created
- Order created with order ID, customer name and city
- Payment cancelled or failed
- Any unhandled exception with full stack trace

All logs use structured properties like {OrderId} and {CustomerName} instead of string concatenation.

## How to Run Locally

Requirements:
- .NET 10 SDK
- SQL Server LocalDB (included with Visual Studio)
- A free Stripe account with test keys

Steps:
1. Clone the repository
2. Open the solution in Visual Studio
3. Right-click SportsStore and click Manage User Secrets, then add your Stripe test keys
4. Open the terminal, go to the SportsStore folder and run:
   dotnet ef database update --context StoreDbContext
   dotnet ef database update --context AppIdentityDbContext
5. Press F5 to run the application
6. To run the tests, right-click the test project and click Run Tests
