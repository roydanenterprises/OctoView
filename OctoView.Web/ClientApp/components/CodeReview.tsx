import * as React from 'react'; 

export interface ICodeReview{
	name : string;
	status: string;
	url: string;
}

export class CodeReview extends
React.Component<ICodeReview, any> {
	render() {
		return <div></div>;
	}
}