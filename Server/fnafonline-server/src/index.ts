import { WebSocketServer } from "ws";

const server = new WebSocketServer({
    port: 8080,
});

server.on("connection", (socket, socketMessage) => {
    let name = socketMessage.socket.remoteAddress;
    console.log(`[${name}] connected`);

    socket.on("message", (message, isBinary) => {
        console.log(`[${name}] messages`, message.toString("utf8"));
    });

    socket.on("close", (e) => {
        console.log(`[${name}] disconnected (${e})`);
    });
});

server.on("listening", () => {
    console.log("server started");
});
