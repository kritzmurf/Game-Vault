interface PaginationProps {
    page: number
    totalPages: number
    onPageChange: (page: number) => void
}

function Pagination({ page, totalPages, onPageChange}: PaginationProps) {
    if (totalPages <= 1) return null

    return (
        <div className="flex items-center justify-center gap-4 mt-8">
            <button
                onClick={() => onPageChange(page - 1)}
                disabled={page <= 1}
                className=" px-4 py-2 bg-gray-800 rounded-lg hover:bg-gray-700
                            disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
                ← Prev
            </button>
            <span className="text-gray-400">
                Page {page} of {totalPages}
            </span>
            <button
                onClick={() => onPageChange(page + 1)}
                disabled={page >= totalPages}
                className=" px-4 py-2 bg-gray-800 rounded-lg hover:bg-gray-700
                            disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
            >
                Next →
            </button>
        </div>
    )
}

export default Pagination
