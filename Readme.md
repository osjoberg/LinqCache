LinqCache
=========
LinqCache is a simple yet poweful framework for caching LINQ queries. 
The framework works seamlessly wheter the backing data store is objects, XML, LINQ-to-SQL, Entity Framework, NHibernate or any other LINQ enabled provider. 
You can also extend the frame work with your own custom cache containers or custom cache invalidation rules.

Install via NuGet
-----------------
To install LinqCache, run the following command in the Package Manager Console:
```
PM> Install-Package LinqCache
```
Examples
--------
+ Cache LINQ to objects query indefinetly
```C#
var query = customers.Where(customer => customer.Name == "John").AsCached();
```

+ Cache LINQ to sql query indefinetly
```C#
using (var context = new MyDbContext())
{
	var customers = customers.Where(customer => customer.Name.StartsWith("A").AsCached());
}
```

+ Cache LINQ to sql query for ten minutes
```C#
using (var context = new MyDbContext())
{
	var customers = customers.Where(customer => customer.Name.StartsWith("A")).AsCached(new DurationInvalidator(TimeSpan.FromMinutes(10));
}
```
+ Invalidate LINQ to sql query
```C#
using (var context = new MyDbContext())
{
	var customers = customers.Where(customer => customer.Name.StartsWith("A").AsCached());
}

.
.
.

customers.Invalidate();
```
+ Invalidate whole cache
```C#
LinqCacheConfiguration.Container.Clear();
```

Supported Cache Containers
--------------------------
+ MemoryCache, Managed in-process memory cache. (Default)

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
+ Fixed broken SqlDependency LinqToSql support
+ Detect unsupported queries for SqlDependency
+ Detect changetracking not running for SqlDependency
+ Invalidator.OnInit should only execute once
+ Patching of context should be done on every request if neccessary

**Future**
+ Entity framework support for SqlDependency
+ Proper support for caching of scalar queries
+ Proper support for pre compiled queries