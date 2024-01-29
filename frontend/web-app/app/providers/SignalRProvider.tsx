"use client";

import { useAuctionStore } from "@/hooks/useAuctionStore";
import { useBidStore } from "@/hooks/useBidStore";
import { Bid, Auction, AuctionFinished } from "@/types";
import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { User } from "next-auth";
import { ReactNode, useEffect, useState } from "react";
import toast from "react-hot-toast";
import AuctionCreatedToast from "../components/AuctionCreatedToast";
import AuctionFinishedToast from "../components/AuctionFinishedToast";
import { getDetailedViewData } from "../actions/auctionActions";

type Props = {
  children: ReactNode;
  user: User | null;
};

export default function SignalRProvider({ children, user }: Props) {
  const [connection, setConnection] = useState<HubConnection | null>(null);
  const setCurrentPrice = useAuctionStore((state) => state.setCurrentPrice);
  const addBid = useBidStore((state) => state.addBid);
  const apiUrl =
    process.env.NODE_ENV === "production"
      ? "https://api.carsties.com/notifications"
      : process.env.NEXT_PUBLIC_NOTIFY_URL;

  useEffect(() => {
    const newConnection = new HubConnectionBuilder()
      .withUrl(apiUrl!)
      .withAutomaticReconnect()
      .build();

    setConnection(newConnection);
  }, [apiUrl]);

  useEffect(() => {
    if (connection) {
      connection
        .start()
        .then(() => {
          console.log("Connected to the notifications hub");

          connection.on("BidPlaced", (bid: Bid) => {
            if (bid.bidStatus.includes("Accepted")) {
              setCurrentPrice(bid.auctionId, bid.amount);
            }
            addBid(bid);
          });

          connection.on("AuctionCreated", (auction: Auction) => {
            if (user?.username !== auction.seller) {
              return toast(<AuctionCreatedToast auction={auction} />, {
                duration: 8000,
              });
            }
          });

          connection.on(
            "AuctionFinished",
            async (finishedAction: AuctionFinished) => {
              const auction = getDetailedViewData(finishedAction.auctionId);
              return toast.promise(
                auction,
                {
                  loading: "Loading",
                  success: (auction) => (
                    <AuctionFinishedToast
                      finishedAuction={finishedAction}
                      auction={auction}
                    />
                  ),
                  error: (err) => "Auction finished",
                },
                { success: { duration: 8000, icon: null } }
              );
            }
          );
        })
        .catch((error) => console.log(error));
    }

    return () => {
      connection?.stop();
    };
  }, [connection, setCurrentPrice, addBid, user]);

  return children;
}
