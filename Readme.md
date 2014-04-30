LinqCache
=========
LinqCache is a simple yet poweful caching framework for LINQ queries. 
The framework works seamlessly weiter the backing data store is objects, XML, LINQ-to-SQL, Entity Framework, NHibernate or any other LINQ enabled provider.
The framework is also easy to extend with custom cahce container and cache invalidation rules.

Caching Examples
----------------
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

Invalidation Examples
---------------------
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
LinqCacheConfiguration.Container.RemoveAll();
```

Supported cache containers
--------------------------
+ MemoryCache, Managed in-process memory cache. (Default)

Supported invalidation rules
----------------------------
+ ManualInvalidator, Items are invalidated manually. (Default)
+ DurationInvalidator, Items are invalidated after a specified duration.
+ SlidingInvalidator, Items are invalidated after a specified duration. If the item is queried from the cache, then the item duration will be reset to the specified duration again, keeping used data cached longer.
+ DateTimeInvalidator, Items are invalidated at a specific date and time.


Configuration
-------------
By default .NET MemoryCache will be used for in-process caching with manual expiry of cached items.

You can change the defaults by modifying the LinqCacheConfiguration.Default property. If you need specific configrations in different places of the code you can pass a new configuration into the AsCached method.

Credits
-------
Expression parsing is based on: http://petemontgomery.wordpress.com/2008/08/07/caching-the-results-of-linq-queries/ by Pete Montgomery.