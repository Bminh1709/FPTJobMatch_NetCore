"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
var userId = $("#userId").val();
let canDisplayMessage = localStorage.getItem('canDisplayMessage');

// Toggle Chat
$('#chat_btn').on('click', function () {
    var userId = $('#userId').val();
    if (userId == null) {
        window.location.href = '/access';
    } else {
        connection.invoke("IsAdminOnline").then(function (isAdminOnline) {
            if (isAdminOnline) {
                $('#chat_onlineIcon').show();
            } else {
                $('#chat_onlineIcon').hide();
            }
        }).catch(function (err) {
            console.error(err.toString());
        });
        $('#chat_box').toggle();
    }
});

// Toggle Chat for Admin
$('#chat_btn_admin').on('click', function () {
    $('#chat_box_admin').toggle();
});


// Disable the send button until connection is established.
$("#sendButton").prop("disabled", true);

// Start the connection
connection.start().then(function () {
    $("#sendButton").prop("disabled", false);
}).catch(function (err) {
    console.error(err.toString());
});

// Add to queue if user starts the chat
$("#chat_start").on('click', function (event) {
    connection.invoke("IsAdminOnline").then(function (isAdminOnline) {
        if (isAdminOnline) {
            $("#messageInput").removeAttr("disabled");
            $("#chat_start").remove();
            localStorage.setItem('canDisplayMessage', true);
        } else {
            $("#messagesList").append(`<p class="bg-white p-3 w-fit rounded-xl">Admin is not available now</p>`);
            localStorage.setItem('canDisplayMessage', false);
        }
    }).catch(function (err) {
        console.error(err.toString());
    });
    connection.invoke("AddToQueue", userId).catch(function (err) {
        console.error(err.toString());
    });
    event.preventDefault();
});

// Dequeue if user ends the chat
$("#chat_end").on('click', function () {
    debugger;
    // Delete chat messages from local storage
    localStorage.removeItem('chatMessages');
    localStorage.setItem('canDisplayMessage', false);
    // Invoke method to remove user from the queue
    connection.invoke("RemoveFromQueue", userId).then(function () {
        // Reload the page after successfully removing from the queue
        location.reload();
    }).catch(function (err) {
        console.error(err.toString());
    });
});

// Method to update the chat number display
connection.on("UpdateChatNumber", function (chatNumber) {
    $("#chat_number").text("Your turn waiting order is #" + chatNumber);
});


// Method to receive and display messages
connection.on("ReceiveMessage", function (message) {
    localStorage.setItem('canDisplayMessage', true);

    $("#messagesList").append(`<p class="bg-white p-3 w-fit rounded-xl">${message}</p>`);

    // Save the received message to localStorage
    saveMessageToLocal("Receiver", message);
});

// Function to save message to localStorage
function saveMessageToLocal(sender, message) {
    let messages = JSON.parse(localStorage.getItem('chatMessages')) || [];
    messages.push({ sender: sender, text: message });
    localStorage.setItem('chatMessages', JSON.stringify(messages));
}


$("#sendButton").on('click', function (event) {
    // Get admin Id (Destination user)
    var message = $("#messageInput").val();

    $("#messagesList").append(`<div class="flex gap-2"><p class="bg-MyOrange text-white p-3 w-fit rounded-xl ml-auto">${message}</p></div>`);

    // Save the sent message to localStorage
    saveMessageToLocal("Sender", message);

    connection.invoke("SendMessage", message).then(function () {
        // Empty message box
        $("#messageInput").val('');
    }).catch(function (err) {
        console.error(err.toString());
    });
    event.preventDefault();
});

// Send message admin
$("#sendButton_admin").on('click', function (event) {
    // Get admin Id (Destination user)
    var message = $("#messageInput").val();

    $("#messagesList").append(`<div class="flex gap-2"><p class="bg-MyOrange text-white p-3 w-fit rounded-xl ml-auto">${message}</p></div>`);

    // Save the sent message to localStorage
    saveMessageToLocal("Sender", message);

    connection.invoke("SendMessageToUser", message).then(function () {
        // Empty message box
        $("#messageInput").val('');
    }).catch(function (err) {
        console.error(err.toString());
    });
    event.preventDefault();
});


// Function to display stored messages from localStorage
function displayStoredMessages() {
    let messages = JSON.parse(localStorage.getItem('chatMessages')) || [];
    let messagesList = $("#messagesList");
    messagesList.empty(); // Clear existing messages

    $("#messageInput").removeAttr("disabled");

    messages.forEach(message => {
        let messageElement = $("<p>").addClass("p-3 w-fit rounded-xl");
        if (message.sender === 'Receiver') {
            messageElement.addClass('bg-white');
        } else {
            messageElement.addClass('bg-MyOrange text-white ml-auto');
        }
        messageElement.text(message.text);
        messagesList.append(messageElement);
    });
}

// Call the function to display stored messages when the page loads
if (JSON.parse(canDisplayMessage)) {
    displayStoredMessages();
}
