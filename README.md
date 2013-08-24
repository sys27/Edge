Edge
====

A new syntax to create UI (WPF).

## Sample:

```
using SomeNamespace;

Window {
    Title: "kjgkhj",
    Icon: BitmapImage("Icon.ico"),
    Grid {
        ColumnDefinitions: [
            ColumnDefinition { Width: 100 },
            ColumnDefinition
        ],
        TextBox#tb {
            Text: "Hi!!!",
            Style: #st
        },
        TextBlock {
            Text: @tb.Text,
            Grid.Column: 1,
            Style: {
                HorizontalAlignment: Center
            }
        }
        
    },
    
    Resources: [
        Style#st {
            Height: 200
        }
    ]
}
```