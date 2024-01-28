import { Bid } from "@/types";
import { format } from "date-fns";
import React from "react";

type Props = {
  bid: Bid;
};
export default function BidItem({ bid }: Props) {
  function getBidInfo() {
    let bgColor = "";
    let text = "";

    switch (bid.bidStatus) {
      case "Accepted":
        bgColor = "bg-green-200";
        text = "Bid Accepted";
        break;
      case "AcceptedBelowReserve":
        bgColor = "bg-amber-200";
        text = "Reserve not meet";
        break;
      case "TooLow":
        bgColor = "bg-red-200";
        text = "Bid was too low";
        break;
      default:
        bgColor = "bg-red-200";
        text = "Bid placed after auction finished";
        break;
    }

    return { bgColor, text };
  }
  return (
    <div
      className={`
    border-grey-300 border-2 px-3 py-2 rounded-lg flex
    justify-between items-center mb-2 ${getBidInfo().bgColor}
  `}
    >
      <div className="flex flex-col">
        <span>Bidder: {bid.bidder}</span>
        <span className="text-grey-700 text-sm">Time: { format(new Date(bid.bidTime), "dd-MMM-yyyy hh:mm a")}</span>
      </div>
      <div className="flex flex-col text-right">
        <div className="text-xl font-semibold">${bid.amount}</div>
        <div className="flex flex-row items-center">
          <span>{getBidInfo().text}</span>
        </div>
      </div>
    </div>
  );
}
