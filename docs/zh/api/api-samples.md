<p align="center">
 <a href="https://natasha.dotnetcore.xyz/"> 返回 </a> |  <a href="https://natasha.dotnetcore.xyz/en/api/api-samples.html"> English </a>
</p>  

## API用法展示

<br/>  

- **NFunc/NAction**  

```C#

//NFunc 和 NAction 支持：

// 普通方法：      Delegate
// 异步方法：      AsyncDelegate
// 非安全方法：    UnsafeDelegate
// 非安全异步方法： UnsafeAsyncDelegate

var action = NFunc<string, string, Task<string>>.UnsafeAsyncDelegate(@"
                            string result = arg1 +"" ""+ arg2;
                            Console.WriteLine(result);
                            return result;");

string result = await action("Hello", "World1!");
//result = "Hello World1!"
  
```  


<br/>  

- **NewClass/NewInterface/NewStruct**  

```C#

  var result = NewStruct.Create(builder => builder
  
                .Namespace("TestNamespace")
                .OopAccess(AccessTypes.Private)
                .OopName("TestUt2")
                
                .Ctor(item=>item
                    .MemberAccess("public")
                    .Param<string>("name")
                    .Body("this.Name=name;"))
                    
                .OopBody(@"public static void Test(){}")
                
                .PublicStaticField<string>("Name")
                .PrivateStaticField<int>("_age")
                
  );
  
  
  Type type = result.Type;
  var error = result.Exception; 
  
```  

<br/>  

- **NewMethod**  

```C#

 var result = NewMethod.Create<Func<string, string, Task<string>>>(builder => builder
 
                    .UseAsync()
                    .MethodBody(@"
                            string result = arg1 +"" ""+ arg2;
                            Console.WriteLine(result);
                            return result;")
                    );

  var method = result.Method;
  var error = result.Exception; 
  

```  

<br/>  


#### ProxyOperator : [参见UT测试](https://github.com/dotnetcore/Natasha/blob/master/test/NatashaUT/ProxyTest.cs)  

<br/>  


#### OopOperator : [参见UT测试](https://github.com/dotnetcore/Natasha/blob/master/test/NatashaUT/OopTest.cs)  

<br/>  


#### OopComplier : [参见UT测试](https://github.com/dotnetcore/Natasha/blob/master/test/NatashaUT/OopComplierTest.cs)    


<br/>  

#### CloneOperator   


> 使用该方法可以实现深度克隆

```C#

 var copyInstance = CloneOperator.Clone(instance);

```  

<br/>  

#### SnapshotOperator    

> 使用该方法可以实现快照功能

```C#

 SnapshotOperator.MakeSnapshot(instance);
 //
 //do sth
 //
 var diff = SnapshotOperator.Compare(instance);

```  

<br/>  

#### CtorOperator    

> 当您知道一个类，并想将它的初始化操作放在委托里

```C#

 var func = CtorOperator.NewDelegate(typeof(Foo));
 Foo instance = func();

```  

<br/>  

<br/>  

#### FastMethodOperator  

  <br/>  

- 普通定制  

> 快速定制一个方法
  
```C#
var action = FastMethodOperator.New
             .Param<string>("str1")
             .Param(typeof(string),"str2")
             .MethodBody("return str1+str2;")
             .Return<Task<string>>()
             .Complie<Func<string,string,string>>();
                    
var result = action("Hello ","World!");    //result:   "Hello World!"
```

<br/>  

- 增强实现与异步支持
> Complie<T>方法会检测参数以及返回类型，如果其中有任何一处没有指定，那么Complie方法会使用自己默认的参数或者返回值进行填充,
如果是Action<int> 这种带有1个参数的，请使用"arg", 另外如果想使用异步方法，请使用UseAsync方法,或者AsyncFrom<Class>(methodName)这两种方法。
返回的参数需要您指定Task<>,以便运行时异步调用，记得外面那层方法要有async关键字哦。

```C#
var delegateAction = FastMethodOperator.New

       .UseAsync()
       .MethodBody(@"
               await Task.Delay(100);
               string result = arg1 +"" ""+ arg2; 
               Console.WriteLine(result);
               return result;")
               
       .Complie<Func<string, string, Task<string>>>();
      
string result = await delegateAction?.Invoke("Hello", "World2!");   //result:   "Hello World2!"
```
<br/>
<br/>  

#### DelegateOperator  
  
> 快速快速实现委托
  
```C# 

//定义一个委托
public delegate string GetterDelegate(int value);
     
     
     
//方法一     
var action = DelegateOperator<GetterDelegate>.Create("value += 101; return value.ToString();");
string result = action(1);              
//result: "102"


//方法二
var action = "value += 101; return value.ToString();".Create<GetterDelegate>();
string result = action(1);              
//result: "102"     

```  

<br/>
<br/>  

#### FakeMethodOperator  

> 快速复制方法并实现

```C#
public class Test
{ 
   public string Handler(string str)
   { 
        retrurn null; 
   }
}

```
```C#
var action = FakeMethodOperator.New
             .UseMethod(typeof(Test).GetMethod("Handler"))
             .StaticMethodContent(" str += "" is xxx;"",return str; ")
             .Complie<Func<string,string>>();
                  
string result = action("xiao");              //result: "xiao is xxx;"          
```  

> [参见UT测试](https://github.com/dotnetcore/Natasha/blob/master/test/NatashaUT/DynamicMethodTest.cs#L96-L196)  

<br/>
<br/>  
