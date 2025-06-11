# **Orders API**

This project implements a **clean, maintainable Orders API** using **C#**, **.NET Web API**, and **Entity Framework Core (EF Core)** with a **SQLite** database. It follows **backend architecture principles**, prioritizes **code quality**, **testing**, and applies **best practices**.

---

##  **Project Structure**

This solution adopts a **layered architecture** with clear separation of concerns. Folder names match their respective project names:

```
OrdersApi/
├── src/                          # Source code
│   ├── OrdersApi.Domain/         # Domain Layer: core entities, business rules
│   ├── OrdersApi.Application/    # Application Layer: use cases, DTOs, orchestration
│   ├── OrdersApi.Infrastructure/ # Infrastructure Layer: EF Core, repositories
│   └── OrdersApi.Web/            # Presentation Layer: API controllers, HTTP DTOs
├── test/                         # Test projects
│   └── OrdersApi.Tests/          # Unit & integration tests
├── OrdersApi.sln                 # Solution file
└── README.md                     # This file
```

---

##  **Setup Instructions**

###  **Clone or Create Project**

If you've **already cloned** this repository, skip this step.  
Otherwise, manually create a **blank solution and projects** in **Visual Studio 2022**, and configure the **project references** accordingly.

---

###  **Copy Code Files**

Copy all provided **C#** and **JSON files** to their corresponding locations as shown in the **Project Structure** section.

---

###  **Install Required NuGet Packages**

In **Visual Studio**:  
Right-click the **Solution** → _Manage NuGet Packages for Solution..._ → Install the following:

#### `OrdersApi.Infrastructure`
- `Microsoft.EntityFrameworkCore.Sqlite`  
- `Microsoft.EntityFrameworkCore.Design`  
- `Microsoft.EntityFrameworkCore.Tools`  

#### `OrdersApi.Application`
- `Microsoft.Extensions.Logging.Abstractions`  

#### `OrdersApi.Web`
- `Swashbuckle.AspNetCore`  
- `Serilog.AspNetCore`  
- `Serilog.Sinks.File`  
- `Microsoft.EntityFrameworkCore.Design`  

#### `OrdersApi.Tests`
- `Microsoft.NET.Test.Sdk`  
- `xunit`  
- `xunit.runner.visualstudio`  
- `coverlet.collector`  
- `Moq`  
- `Microsoft.Extensions.Logging.Abstractions`  

---

###  **Apply Database Migrations**

1. **Delete** `order.db` and all files in `src\OrdersApi.Infrastructure\Migrations`.

2. Open **PowerShell**, navigate to the solution folder.

3. Run the following commands:

```bash
dotnet ef migrations add InitialCreate --project src\OrdersApi.Infrastructure --startup-project src\OrdersApi.Web
dotnet ef database update --project src\OrdersApi.Infrastructure --startup-project src\OrdersApi.Web
```

>  This will create a new **Orders.db** SQLite file under `src/OrdersApi.Web/`.

---

##  **Run the API & Tests**

###  **Running the API**

1. In **Solution Explorer**, right-click `OrdersApi.Web` → _Set as Startup Project_  
2. Click the green **"http" button** in Visual Studio to start the app  
3. Your browser will open **Swagger UI**:  
   [http://localhost:5049/swagger/index.html](http://localhost:5049/swagger/index.html)

4. Use the **POST /api/orders** endpoint:

Sample request JSON:

```json
{
  "orderId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "customerName": "John Doe",
  "items": [
    {
      "productId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
      "quantity": 2
    },
    {
      "productId": "f9e8d7c6-b5a4-3210-fedc-ba9876543210",
      "quantity": 1
    }
  ],
  "createdAt": "2023-10-27T10:00:00Z",
  "quantity": 3
}
```

> Click **Execute** → You should receive an **HTTP 201 Created** response.

---

###  **Running Tests**

1. Go to **Test > Test Explorer** in Visual Studio  
2. Click **Run All Tests**  
3. All tests from `OrderServiceTests` should **pass successfully**

---

##  **Assumptions & Design Decisions**

- **Database Provider**:  
  Chose **SQLite** for local simplicity. For production, **SQL Server** or **PostgreSQL** is recommended.

- **Order ID Generation**:  
  Assumes `OrderId` is provided by the **client** (GUID), supporting **idempotency**.  
  Could be changed to **server-generated** if preferred.

- **No Product Catalog**:  
  `ProductId` is treated as a GUID without validation.  
  In real applications, you'd validate against product inventory.

---

##  **Domain-Driven Design (DDD) Optimization**

-  **Application Layer DTOs** (in `OrdersApi.Application/DTOs`):  
  - No `DataAnnotations`
  - Pure data transport only

-  **Web Layer DTOs** (in `OrdersApi.Web/DTOs`):  
  - Include `DataAnnotations` for **input validation**

-  **Business Rules Enforcement**:  
  - Initially checked in application services  
  - Preferably enforced **inside domain entities** (e.g., via constructor)

---

##  **Error Handling & Logging**

- Basic error handling using `try-catch` and **logging**  
- Consider using **global exception middleware** for advanced scenarios  
- Integrated **Serilog**:  
  - Logs to **console** and **file**  
  - Easy configuration for troubleshooting

---

##  **Async & Test Strategy**

- All I/O operations use `async/await` for **scalability and responsiveness**  
- Unit tests focus on **Application Layer** logic  
- Repositories are **mocked** to **isolate** business logic
---
