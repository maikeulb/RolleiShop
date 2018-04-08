# RolleiShop

E-commerce application where users may browse the catalog, manage their
shopping cart, submit orders (dummy or through stripe), and browse through
their order history. Admin users may manage the catalog (add and edit catalog
items). Features include distributed caching for the catalog items (via Redis),
in-memory catching for the catalog types and brands, localization,
file-uploads, external logging (via Nlog), server-side filtering (via
specification pattern), and emails (via MailKit).

The application is written following a vertically sliced, CQRS architecture
with a rich, encapsulated domain<sup>1</sup> (private collections and setters)
with domain notifications (for sending emails and product price changes). The
application IO is fully-asynchronous and the errors are handled with command
results (similar to F#'s Option Type or Haskell's Maybe monad).

1. The only exceptions to this (as far as I'm aware) are the account classes
   because I didn't want to rewrite the templated classes for integrating with
   Identity.

Technology
----------
* ASP.NET Core
* PostgreSQL (with Entity Framework)
* Redis
* Semantic UI
* Stripe
* Noty
* Rellax

Screenshots
---
### Index 
The index and layout templates are translated to Japanese thanks to Google
translate (Most likely not too accurate).
![index](/screenshots/index.png?raw=true "Index")
***
![japanese](/screenshots/japanese.png?raw=true "Japanese")
### Catalog  
Filter catalog-items by brand and/or format. 
![catalog](/screenshots/catalog.png?raw=true "Catalog")
### Cart
Displays cart items with the abilities to update, clear, and checkout.
Checking out the cart publishes a domain notification to send customer emails.
![cart](/screenshots/cart.png?raw=true "Cart")
***
![mail](/screenshots/mail.png?raw=true "Mail")
### Orders - Details
Displays your detailed order history.
![order](/screenshots/order.png?raw=true "Order")
### Admin 
Admin users may manage the catalog (and inventory). Updating the product price publishes
a domain notification to reflect the price change in the customer's cart.
![admin](/screenshots/admin.png?raw=true "Admin")
***
![add](/screenshots/add.png?raw=true "Add")

Run
---
If you have docker installed,
```
TODO
```
Alternatively, you will need .NET Core 2.0 SDK. If you have the SDK installed,
then open `appsettings.Development.json` and point the connection strings to
your PostgreSQL and Redis servers. Install the javascript dependencies (e.g.
`npm install`). You may optionally fill out the credentials for the mail
server.

`cd` into `./src/RolleiShop` (if you are not already) and run the following:
```
webpack build
dotnet restore
dotnet ef database update -c ApplicationDbContext
dotnet ef database update -c IdentityDbContext
dotnet run
Go to http://localhost:5000
```
NOTE
----
The resources I use to create this project come from several projects and
tutorials provided by Microsoft (mostly eShopOnWeb, eShopOnContainers,
MVCMusicStore, and ContosoUniversity), Jimmy Bogards Contoso University remake
and writing, along with several blogs.

TODO
----
Dockerfile  
Modularize javascript and configure webpack  
Add more unit tests  
Add Serverside sorting by price  
Fix AJAX remove cart-items (low priority)  
ADD compression middleware (maybe)  
Turn off email in DEV mode  
Improve Fluent validation