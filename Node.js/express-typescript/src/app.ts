import express from "express";

const app = express();
const port = 3000;

type Hello = {
  foo: number;
  bar: string;
};

app.get("/", (req, res) => {
  let hello: Hello = {
    foo: 111,
    bar: "bar",
  };
  res.send(hello);
});

app.listen(port, () => {
  if (port === 3000) {
    console.log("true");
  }
  console.log(`server is listening on ${port} !!!`);
});
