## Introduction ##

This is an example that shows the functionality of real-time interaction with Signal-R.
In this example we show how you can make changes in one client, and this changes are reflected in every client connected to the site.

Once deployed the [database](http://code.google.com/p/signalr-cities/downloads/detail?name=cities.rar), change your connection string.
Run proyect and open multiple tabs.

Once running try to delete, update, or add some cities. You will notice that these changes are reflected in all tabs opened.

## The code ##

**in js:**
```
var connection = $.connection('/echo');
connection.received(function (data) {
    // manage received code
});

connection.start();
```

**in c# code:**

initialize connection on global.asax
```
routes.MapConnection<SignalConnection>("echo", "echo/{*operation}");
```

send broadcast to every client:
```
var context = GlobalHost.ConnectionManager.GetConnectionContext<SignalConnection>();
context.Connection.Broadcast(sb.ToString());
```

## Technologies ##

  * SIGNAL-R
  * MVC3
  * SQLSERVER
  * ENTITY-FRAMEWORK
  * BOOTSTRAP CSS FRAMEWORK