import clock from "clock";
import document from "document";

let txtTime = document.getElementById("txtTime");
let txtDate = document.getElementById("txtDate");

clock.granularity = "seconds";
clock.addEventListener("tick", tickHandler);

function tickHandler(evt) {
  let today = evt.date;

  let fullYear = today.getFullYear();
  let monthNameShort = monthsShort[today.getMonth()];
  let dayNumber = today.getDate();

  let hours = zeroPad(today.getHours());
  let mins = zeroPad(today.getMinutes());
  let seconds = zeroPad(today.getSeconds());

  let timeString = `${hours}:${mins}:${seconds}`;
  let dateString = `${dayNumber} ${monthNameShort} ${fullYear}`;

  txtTime.text = timeString;
  txtDate.text = dateString;
}

function zeroPad(i) {
  if (i < 10) {
    i = "0" + i;
  }
  return i;
}

const monthsShort = [
  "JAN",
  "FEB",
  "MAR",
  "APR",
  "MAY",
  "JUN",
  "JUL",
  "AUG",
  "SEP",
  "OCT",
  "NOV",
  "DEC",
];
