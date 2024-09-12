import * as React from 'react';
import './DaysOverdue.scss';
export const DaysOverdue = ({ days, statusColor }) => {
    return <div className='days_overdue'>
        <div className={statusColor}>
            {days}
        </div>
        <div>
            Days
        </div>
        </div>;
};
//# sourceMappingURL=DaysOverdue.jsx.map