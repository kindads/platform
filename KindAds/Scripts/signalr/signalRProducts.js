/*
 * Declare variables
 * */
var IdUser = IdUserIdentity;
var connection = $.hubConnection();
var hubProxy = connection.createHubProxy('notificationHub');


/*
 * Attach client function
 * */
hubProxy.on('showNotification', function (title, message) {
  showNotification(title, message);
});

hubProxy.on('showChatMessage', function (messageHtml, elementToAttach) {
  showChatMessage(messageHtml, elementToAttach);
});

function initializationSignalR() {
  connection = $.hubConnection();
  hubProxy = connection.createHubProxy('notificationHub');

  hubProxy.on('showChatMessage', function (messageHtml, elementToAttach) {
    showChatMessage(messageHtml, elementToAttach);
  });

  connection.start()
    .done(function () {

      //console.log("Signal R connected");
      //console.log("Invoke StartHandlerRx");
      //hubProxy.invoke('StartHandlerRx', IdUser);
      //console.log("IsPublisher: " + IsPublisher);

      if (IsPublisher == 'true') {
        //console.log("Invoke StartHandlerProposal");
        hubProxy.invoke('StartHandlerProposal', IdUser);
      }
      if (InsideChat == true) {
        //console.log("Invoke StartHandlerConversation");
        hubProxy.invoke('StartHandlerConversation', IdUser);
      }
    })
    .fail(function () { console.log('Could not connect'); });
}


/*
 * Details functions
 * */
function showChatMessage(messageHtml, elementToAttach) {
  console.log(messageHtml);
  console.log(elementToAttach);

  $("#" + elementToAttach).append(messageHtml);
}

function showNotification(title, message) {
  //Checar hasta que se libere
  //console.log("currentMessage: " + currentMessage);
  if (currentMessage === 1) {
    setTimeout(function () {
      showNotification(title, message);
    }, 8000);
  }
  else {
    currentMessage = 1;

    //console.log("Titulo:", title);
    //console.log("Message:", message);

    //// Set content in tile
    $("#notification-title").text(title);
    $("#notification-message").text(message);

    //// Show tile
    $("#notification-tile").show("fast");
    //$('#soundNotification')[0].play();

  }
}

/* Attach client function
 * */
hubProxy.on('showError', function (message) {
  //Todo: Show tile and start push notification animation
  alert(message);
});

/*
 * Start communication with Server called StartHandler in SignalRHub.cs
 * */
connection.start()
  .done(function () {
    
    //console.log("Signal R connected");
    //console.log("Invoke StartHandlerRx");
    //hubProxy.invoke('StartHandlerRx', IdUser);
    //console.log("IsPublisher: " + IsPublisher);

    if (IsPublisher == 'true') {
      //console.log("Invoke StartHandlerProposal");
      hubProxy.invoke('StartHandlerProposal', IdUser);
    }
    if (InsideChat == true) {
      //console.log("Invoke StartHandlerConversation");
      hubProxy.invoke('StartHandlerConversation', IdUser);
    }
  })
  .fail(function () { console.log('Could not connect'); });

function getMessageFromSignalRHandler() {
  //console.log("Invoke StartHandlerConversation");  
  connection.start()
    .done(function () {

      //console.log("Signal R connected");
      //console.log("Invoke StartHandlerRx");
      //hubProxy.invoke('StartHandlerRx', IdUser);
      //console.log("IsPublisher: " + IsPublisher);

      if (IsPublisher == 'true') {
        //console.log("Invoke StartHandlerProposal");
        hubProxy.invoke('StartHandlerProposal', IdUser);
      }
      if (InsideChat == true) {
        //console.log("Invoke StartHandlerConversation");
        hubProxy.invoke('StartHandlerConversation', IdUser);
      }
    })
    .fail(function () { console.log('Could not connect'); });
}

/*
 * Wait method
 * */
function wait(ms) {
  var start = new Date().getTime();
  var end = start;
  while (end < start + ms) {
    end = new Date().getTime();
  }
}
