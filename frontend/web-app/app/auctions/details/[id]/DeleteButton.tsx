'use client'

import { deleteAuction } from "@/app/actions/auctionActions";
import { Button } from "flowbite-react";
import { useRouter } from "next/navigation";
import React, { useState } from "react";
import toast from "react-hot-toast";

type Props = {
  id: string;
};

export default function DeleteButton({ id }: Props) {
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  function doDelete() {
    setLoading(true);
    deleteAuction(id)
      .then((res) => {
        if (res) throw res.error;
        router.push("/auctions");
      })
      .catch(error => {
        console.log(error)
        toast.error("Error deleting auction");
      })
      .finally(() => setLoading(false));
  }

  return (
    <Button color='failure' isProcessing={loading} onClick={doDelete}>
      Delete Auction
    </Button>
  );
}
