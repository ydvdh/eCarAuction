'use server'

import { Auction, PagedResult } from "@/types";

export async function getData(query: string): Promise<PagedResult<Auction>> {
    const response = await fetch(`http://localhost:6001/api/search${query}`)
    if (!response.ok) throw new Error('Failed to fetch data')

    return response.json();
}

export async function updateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 100000) + 1
    }

    const res = await fetch('http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', {
        method: 'PUT',
        headers: {},
        body: JSON.stringify(data)
    });
    if(!res.ok) return {status: res.status, message: res.statusText}

    return res.statusText;
}