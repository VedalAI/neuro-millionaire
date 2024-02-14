import express from "express";
import {urlencoded} from "body-parser";
import {send} from "./index";

const app = express();
app.use(urlencoded({extended: false}));
app.listen(1337);

app.get("/answer", (req, res) => {
    send({"command": "millionaire/answer", data: {"answer": req.body.answer}});
    res.sendStatus(200);
});

app.get("/lifeline", (req, res) => {
    send({command: "millionaire/lifeline", data: {"lifeline": req.body.lifeline}});
    res.sendStatus(200);
})

app.get("/asktheaudience", (req, res) => {
    send({command: "millionaire/lifeline/ask_the_audience/results", data: {
        "percentageA": parseInt(req.body.a),
        "percentageB": parseInt(req.body.b),
        "percentageC": parseInt(req.body.c),
        "percentageD": parseInt(req.body.d),
    }});
    res.sendStatus(200);
});

app.get("/phoneafriend", (req, res) => {
    send({command: "millionaire/lifeline/phone_a_friend/results", data: {
        "result": req.body.answer
    }});
    res.sendStatus(200);
});
