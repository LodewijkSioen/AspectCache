AspectCache
===========

Caching With Attributes. Powered by [Castle Windsor](http://www.castleproject.org). Like this:

```c#
[Cache]
public string SomeMethod(int someArgument)
{
  return "somevalue";
}

```

See my [blogpost](http://www.correlatedcontent.com/blog/AOP-Caching-With-Castle-Windsor/) for more information.
