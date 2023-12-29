'use client'

import React, { useEffect, useState } from 'react'
import AuctionCard from './AuctionCard';
import { Auction, PagedResult } from '@/types';
import AppPagination from '../components/AppPagination';
import { getData } from '../actions/auctionActions';
import Filters from './Filters';
import { useParamsStore } from '../hooks/useParamsStore';
import { shallow } from 'zustand/shallow';
import qs from 'query-string';

export default  function Listings() {
    const [data, setData] = useState<PagedResult<Auction>>()
    const params = useParamsStore(state=> ({
        pageNumber: state.pageNumber,
        pageSize: state.pageSize,
        searchTerm: state.searchTerm,
        orderBy: state.orderBy,
        filterBy: state.filterBy
    }), shallow)

    const setParams = useParamsStore(state=>state.setParams)
    const url = qs.stringifyUrl({url: '', query:params})

    function setPageNumber(pageNumber: number){
        setParams({pageNumber})
    }

    useEffect(()=>{
        getData(url).then(data=>{
           setData(data)
        })
    }, [url]);

    if(!data) return <h3>Loading...</h3>

    return (
        <>
            <Filters />
            <div className='grid grid-cols-4 gap-6'>
                { data.result.map(auction => (
                    <AuctionCard auction={auction} key={auction.id} />
                ))}
            </div>
            <div className='flex justify-center mt-5'>
                <AppPagination currentPage={params.pageNumber} pageChanged={setPageNumber} pageCount={data.pageCount} />
            </div>
        </>
    )
}
