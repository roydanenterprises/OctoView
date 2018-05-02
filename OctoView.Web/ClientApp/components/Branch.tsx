import * as React from 'react';
import {IPullRequest, PullRequest } from './PullRequest'

export interface IBranch {
	repo: string;
	branchName: string;
	layout: string;
	pulls: IPullRequest[];
}

export interface IBranchState {
	isOpen: boolean;
}

/*export const Feature: React.SFC<IFeature> = (props) => {
	if (props.layout === "table") {
		return <div>
			       <div>{props.repo}</div>
		       </div>;
	}
	return <div>
		       <div>{props.repo}</div>
	       </div>;
};*/

export class Branch extends
	React.Component<IBranch, any> {
	componentWillMount() {
		this.setState({ isOpen: false });
	}
	toggleState() {
		this.setState({ isOpen: !this.state.isOpen });
	}
	render() {
		
		let className = 'ov-c-branch';
		if (this.state.isOpen) {
		  className += ' ov-c-branch--opened';
		}

		if (this.props.layout === 'table') {
			return <div>
				       <div>{this.props.repo}</div>
			       </div>;
		}
		return <div onClick={() => this.toggleState()} className={className}>
					<div className="ov-c-branch__left-indicator"></div>
			    	<div className="ov-c-branch__name">{this.props.branchName}</div>
			<div className="ov-c-branch__approval-indicator"></div>
			    	<div className="ov-c-branch__pull-request-drawer">
						<div className="ov-c-branch__pull-request-drawer-contents">
							{this.props.pulls.map(pulls => <PullRequest {...pulls}/>)}
						</div>
			    	</div>
		       </div>;
	}
}