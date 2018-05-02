import * as React from 'react';

export interface ICodeReview {
	name: string;
	status?: string;
	url?: string;
	avatarUrl?: string;
}

export interface ICodeReviewState {
	status? : string;
}

export class CodeReview extends
	React.Component<ICodeReview, ICodeReviewState> {
	constructor() {
		super();
	}

	componentWillMount() {
		this.setState({ status: this.props.status });
	}

	stateChange() {
		this.setState({ status: 'Other State' });
	}
	render() {
		console.log('rendered code review.');
		return <div>
			<button onClick={() => this.stateChange()}>Update State</button>
			       <div>{this.props.name}</div>
			       <div>{this.state.status}</div>
			       <div>{this.props.url}</div>
		       </div>;
	}
}