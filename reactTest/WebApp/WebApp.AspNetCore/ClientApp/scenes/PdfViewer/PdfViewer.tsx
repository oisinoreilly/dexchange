import * as React from 'react';
import { ReduxProps, withRedux } from '../../shared/withReduxStore';
import { withRouter } from 'react-router-dom';
import PDF from 'react-pdf-js';

type props = ReduxProps & {                                
    name: string,
    type: string;
    match: any;
    history: any;
};   
type state = {
    page: number;
    pages: number;
};

export class PdfViewer extends React.Component<props, state>{
    componentDidMount() {
        const { actions, match, history } = this.props;
        this.state = {page: 0, pages: 0};
    }

    onDocumentComplete = (pages) => {
        this.setState({ page: 1, pages });
      }
    
      onPageComplete = (page) => {
        this.setState({ page });
      }
    
      handlePrevious = () => {
        this.setState({ page: this.state.page - 1 });
      }
    
      handleNext = () => {
        this.setState({ page: this.state.page + 1 });
      }

    render() {
        //Convert base64 string to binary
        var pdfData = atob('JVBERi0xLjcKCjEgMCBvYmogICUgZW50cnkgcG9pbnQKPDwKICAvVHlwZSAvQ2F0YWxvZwogIC9QYWdlcyAyIDAgUgo+PgplbmRvYmoKCjIgMCBvYmoKPDwKICAvVHlwZSAvUGFnZXMKICAvTWVkaWFCb3ggWyAwIDAgMjAwIDIwMCBdCiAgL0NvdW50IDEKICAvS2lkcyBbIDMgMCBSIF0KPj4KZW5kb2JqCgozIDAgb2JqCjw8CiAgL1R5cGUgL1BhZ2UKICAvUGFyZW50IDIgMCBSCiAgL1Jlc291cmNlcyA8PAogICAgL0ZvbnQgPDwKICAgICAgL0YxIDQgMCBSIAogICAgPj4KICA+PgogIC9Db250ZW50cyA1IDAgUgo+PgplbmRvYmoKCjQgMCBvYmoKPDwKICAvVHlwZSAvRm9udAogIC9TdWJ0eXBlIC9UeXBlMQogIC9CYXNlRm9udCAvVGltZXMtUm9tYW4KPj4KZW5kb2JqCgo1IDAgb2JqICAlIHBhZ2UgY29udGVudAo8PAogIC9MZW5ndGggNDQKPj4Kc3RyZWFtCkJUCjcwIDUwIFRECi9GMSAxMiBUZgooSGVsbG8sIHdvcmxkISkgVGoKRVQKZW5kc3RyZWFtCmVuZG9iagoKeHJlZgowIDYKMDAwMDAwMDAwMCA2NTUzNSBmIAowMDAwMDAwMDEwIDAwMDAwIG4gCjAwMDAwMDAwNzkgMDAwMDAgbiAKMDAwMDAwMDE3MyAwMDAwMCBuIAowMDAwMDAwMzAxIDAwMDAwIG4gCjAwMDAwMDAzODAgMDAwMDAgbiAKdHJhaWxlcgo8PAogIC9TaXplIDYKICAvUm9vdCAxIDAgUgo+PgpzdGFydHhyZWYKNDkyCiUlRU9G');
        
        var binaryContent = {data: pdfData};

        //var file="dist/test.pdf"
        return <div>
            <PDF
            binaryContent={binaryContent}
            onDocumentComplete={this.onDocumentComplete}
            onPageComplete={this.onPageComplete}
            page={1}
          />
            </div>
    }
}

export default withRouter(withRedux(PdfViewer));