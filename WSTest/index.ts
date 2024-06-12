import { WebSocket, WebSocketServer } from "ws";
import "./app";

const wss = new WebSocketServer({ port: 8000 });

let connections : WebSocket[] = [];

wss.on("connection", function connection(ws) {
    ws.once("message", (data) => {
        const obj = JSON.parse(data.toString()) as Message;
        console.log(obj);
        if (obj.Command == "client") {
            console.log("Registered new client");
            connections.push(ws);
            ws.on("message", data => onMessageReceived(JSON.parse(data.toString()) as Message));
            ws.on("close", () => connections = connections.filter(c => c != ws));
        } else {
            ws.close();
        }
    });
});

function onMessageReceived(data : Message) {
    console.log(data);
}

export function send(msg : Message) {
    const msgStr = JSON.stringify(msg);
    for (const connection of connections) {
        connection.send(msgStr);
    }
}

export type Message = {
    Command: string,
    Data?: {[key: string]:any}
}
