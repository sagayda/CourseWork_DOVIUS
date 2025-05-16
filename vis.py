import pandas as pd
import matplotlib.pyplot as plt
import sys
import json
from matplotlib.ticker import MultipleLocator
import os

def print_python_usage():
    print("Usage: lua generate_task.lua <params> --format=json | python plot_points.py [--save <output_filename>]")
    print("Arguments:")
    print("  --save <output_filename>: Save the plot to the specified file instead of displaying it interactively.")
    print("                            File format is inferred from the extension (.png, .jpg, .svg, .pdf, etc.).")
    print("\nExample:")
    print("  lua generate_task.lua 0 100 0 100 50 10 --format=json | python plot_points.py --save task_plot.png")
    print("  lua generate_task.lua 0 10 0 10 10 3 --format=json | python plot_points.py # Display interactively")

# --- Обработка аргументов командной строки ---
output_filename = None
i = 1
while i < len(sys.argv):
    if sys.argv[i] == "--save":
        if i + 1 < len(sys.argv):
            output_filename = sys.argv[i + 1]
            i += 1
        else:
            print("Error: --save argument requires a filename.")
            print_python_usage()
            sys.exit(1)
    else:
        print(f"Warning: Unknown argument ignored: {sys.argv[i]}")
    i += 1

print("Reading JSON data from standard input...")
try:
    data = json.load(sys.stdin)
    points = data.get("Points", [])

    if not points:
        print("No 'Points' data found in JSON.")
        sys.exit(0)

    # Преобразуем список словарей в DataFrame
    df = pd.DataFrame([
        {
            'x': point['Coordinates']['X'],
            'y': point['Coordinates']['Y'],
            'deliveries': point['Deliveries']
        }
        for point in points
    ])

except json.JSONDecodeError as e:
    print(f"Error parsing JSON input: {e}")
    print_python_usage()
    sys.exit(1)
except Exception as e:
    print(f"Error processing JSON data: {e}")
    sys.exit(1)

if df.empty:
    print("No data points found after processing JSON.")
    sys.exit(0)

print(f"Successfully read {len(df)} data points.")

# --- Визуализация ---
plt.figure(figsize=(10, 8))
ax = plt.gca()

scatter = ax.scatter(df['x'], df['y'],
                     s=df['deliveries'] * 25 + 10,
                     c=df['deliveries'],
                     cmap='viridis',
                     alpha=0.7,
                     edgecolors='w',
                     linewidth=0.5)

cbar = plt.colorbar(scatter)
cbar.set_label('Number of Deliveries')

plt.xlabel('X Coordinate')
plt.ylabel('Y Coordinate')
plt.title('Task Points Visualization')

ax.xaxis.set_major_locator(MultipleLocator(5))
ax.yaxis.set_major_locator(MultipleLocator(5))
ax.xaxis.set_minor_locator(MultipleLocator(1))
ax.yaxis.set_minor_locator(MultipleLocator(1))

plt.grid(True, which='minor', linestyle='--', alpha=0.6)
plt.grid(True, which='major', linestyle='-', alpha=0.8)
ax.set_aspect('equal', adjustable='box')

# --- Сохранение или отображение ---
if output_filename:
    try:
        output_dir = os.path.dirname(output_filename)
        if output_dir and not os.path.exists(output_dir):
            os.makedirs(output_dir)
            print(f"Created directory: {output_dir}")
        plt.savefig(output_filename, bbox_inches='tight', dpi=300)
        print(f"Plot saved successfully to {output_filename}")
    except Exception as e:
        print(f"Error saving plot to {output_filename}: {e}")
        sys.exit(1)
else:
    print("Displaying plot interactively. Close the window to exit.")
    plt.show()
