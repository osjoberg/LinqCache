LinqCache
=========
LinqCache is a simple yet poweful framework for caching LINQ queries. 
The framework works seamlessly whether the backing data store is objects, XML, LINQ-to-SQL, Entity Framework, NHibernate or any other LINQ enabled provider. 
You can also extend the framework with your own custom cache containers or custom cache invalidation rules.

Install via NuGet
-----------------
To install LinqCache, run the following command in the Package Manager Console:
```
PM> Install-Package LinqCache
```

You can also view the [package page](http://www.nuget.org/packages/LinqCache/) on NuGet.

Examples
--------
+ Cache LINQ to objects query indefinitely
```C#
var query = customers
	.Where(customer => customer.Name == "John")
	.AsCached();
```

+ Cache LINQ-to-SQL / Entity Framework query indefinitely
```C#
using (var context = new MyDbContext())
{
	var customers = context.Customers
		.Where(customer => customer.Name.StartsWith("A")
		.AsCached());
}
```

+ Cache LINQ-to-SQL / Entity Framework query for ten minutes
```C#
using (var context = new MyDbContext())
{
	var customers = context.Customers
		.Where(customer => customer.Name.StartsWith("A"))
		.AsCached(new DurationInvalidator(TimeSpan.FromMinutes(10));
}
```
+ Invalidate LINQ-to-SQL / Entity Framework query
```C#
using (var context = new MyDbContext())
{
	var customers = context.Customers
		.Where(customer => customer.Name.StartsWith("A")
		.AsCached());
	.
	.
	.
	customers.Invalidate();
}

```
+ Invalidate whole cache
```C#
LinqCacheConfiguration.Default.Container.Clear();
```

Supported Cache Containers
--------------------------
+ MemoryCacheContainer, Managed in-process memory cache. (Default)

Supported Invalidation Rules
----------------------------
+ ManualInvalidator, Items are invalidated manually. (Default)
+ DurationInvalidator, Items are invalidated after a specified duration.
+ SlidingInvalidator, Items are invalidated after a specified duration. If the item is queried from the cache, then the item duration will be reset to the specified duration again, keeping used data cached longer.
+ DateTimeInvalidator, Items are invalidated at a specific date and time.
+ SqlDependencyInvalidator, Items are invalidated automatically when underlying data is changed in the database.

Configuration
-------------
By default .NET MemoryCache will be used for in-process caching with manual expiry of cached items.

You can change the default configuration by modifying the LinqCacheConfiguration.Default property. If you need to override the defaults in different places of the code you can pass a invalidation rule or a container to the AsCached method.

Credits
-------
Expression parsing is based on: http://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/ by Pete Montgomery.


Version history
---------------
**Version 0.1.0**
+ Initial version

**Version 0.1.1**
+ Minor fixes

**Version 0.1.2**
+ Fixed broken SqlDependency LINQ-to-SQL support
+ Detect unsupported queries for SqlDependency
+ Detect changetracking not running for SqlDependency
+ Invalidator.OnInit should only execute once
+ Patching of context should be done on every request if neccessary

**Version 0.1.3**
+ Removed unused EntityFramework dependency

**Version 0.1.4**
+ Added manual cache key support
+ Added benchmark tests

**Version 0.1.5**
+ SqlDependencyInvalidator now supports Entity Framework

**Version 0.2.0**
+ Proper support for caching of scalar queries
+ Now properly implementing IQueryable<T> and IQueryProvider

**Version 0.2.1**
+ Added unit tests for Entity Framework Database First

**Version 0.2.2**
+ Removed uneccessary class constraint
+ Added support for caching of projections

**Future**
+ Proper support for pre-compiled queries
+ Support for distributed caching via REDIS or some other established key/value storage engine