import * as React from 'react';

export interface LayoutProps {
    children?: React.ReactNode;
}

export class Layout extends React.Component<LayoutProps, {}> {
    public render() {
        return  <div className='row'> 
                    { this.props.children }
                </div>;
    }
}
