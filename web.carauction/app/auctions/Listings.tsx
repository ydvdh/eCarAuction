import React from 'react'
import AuctionCard from './AuctionCard';

async function getData() {
    const response = await fetch('http://localhost:6001/api/search?pageSize=10')
    if (!response.ok) throw new Error('Failed to fetch data')

    return response.json();
}

export default async function Listings() {
    const data = await getData();
    return (
        <div className='grid grid-cols-4 gap-6'>
            {data && data.result?.map((auction: any) => (
                <AuctionCard auction={auction} key={auction.id} />
            ))}
        </div>
    )
}
