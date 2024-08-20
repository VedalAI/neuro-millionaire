import { WebSocket, WebSocketServer } from "ws";
import "./app";

const wss = new WebSocketServer({ port: 8000 });

let connections : WebSocket[] = [];

wss.on("connection", function connection(ws) {
    ws.once("message", (data) => {
        const obj = JSON.parse(data.toString()) as Message;
        console.log("<---", obj);
        if (obj.Command == "client") {
            connections.push(ws);
            ws.on("message", data => onMessageReceived(JSON.parse(data.toString()) as Message));
            ws.on("close", () => connections = connections.filter(c => c != ws));
        } else {
            ws.close();
        }
    });
});

async function onMessageReceived(data : Message) {
    console.log("<---", data);

    if (data.Command == "launched") {
        send({
            Command: "language",
            Data: {
                language: "EN_US",
            }
        })
    } else if (data.Command == "ready") {
        send({
            Command: "start",
            Data: {
                difficulty: "normal",
                categories: data.Data.availableCategories
            }
        });
    } else if (data.Command == "character") {
        await new Promise(resolve => setTimeout(resolve, 500));
        send({
            Command: "character/response",
            Data: {
                accept: Math.random() > 0.5
            }
        });
    }
}

export function send(msg : Message) {
    console.log("--->", msg);

    const msgStr = JSON.stringify(msg);
    for (const connection of connections) {
        connection.send(msgStr);
    }
}

export type Message = {
    Command: string,
    Data?: {[key: string]:any}
}
